using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using SmartSql.Configuration;
using SmartSql.Configuration.Maps;
using SmartSql.Utils;

namespace Shriek.Extensions.SmartSql
{
    public class NativeConfigLoader : ConfigLoader
    {
        private readonly ILogger _logger;
        private const int DELAYED_LOAD_FILE = 500;
        private readonly SmartSqlOptions _smartSqlOptions;

        public string ConnectString { get; private set; }

        public NativeConfigLoader(string connectString) : this(NullLoggerFactory.Instance, connectString)
        {
        }

        public NativeConfigLoader(ILoggerFactory loggerFactory, string connectString)
        {
            ConnectString = connectString;
        }

        public NativeConfigLoader(ILoggerFactory loggerFactory, SmartSqlOptions options)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _smartSqlOptions = options;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override SmartSqlMapConfig Load(string path, ISmartSqlMapper smartSqlMapper)
        {
            _logger.LogDebug($"NativeConfigLoader Load: { _smartSqlOptions.SqlMapperPath} Starting");

            var config = new SmartSqlMapConfig()
            {
                Path = _smartSqlOptions.SqlMapperPath,
                SmartSqlMapper = smartSqlMapper,
                SmartSqlMaps = new List<SmartSqlMap>(),
                SmartSqlMapSources = new List<SmartSqlMapSource>()
                {
                    new SmartSqlMapSource()
                    {
                        Path = _smartSqlOptions.SqlMapperPath,
                        Type = SmartSqlMapSource.ResourceType.Directory
                    }
                },
                Database = new Database()
                {
                    DbProvider = new DbProvider()
                    {
                        ParameterPrefix = _smartSqlOptions.ParameterPrefix,
                        Name = _smartSqlOptions.DbProviderFactory.GetType().Name,
                        Type = _smartSqlOptions.DbProviderFactory.GetType().AssemblyQualifiedName
                    },
                    WriteDataSource = new WriteDataSource()
                    {
                        ConnectionString = _smartSqlOptions.ConnectionString,
                        Name = _smartSqlOptions.LoggingName
                    },
                    ReadDataSources = new List<ReadDataSource>()
                    {
                        new ReadDataSource()
                        {
                            ConnectionString = _smartSqlOptions.ConnectionString,
                            Name = _smartSqlOptions.LoggingName,
                            Weight = 1,
                        }
                    },
                },
                Settings = new Settings()
                {
                    ParameterPrefix = _smartSqlOptions.ParameterPrefix,
                    IsWatchConfigFile = true
                },
            };

            if (_smartSqlOptions.UseManifestResource)
            {
                foreach (var sourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                {
                    LoadManifestSmartSqlMap(config, sourceName);
                }
            }
            else
            {
                foreach (var sqlmapSource in config.SmartSqlMapSources)
                {
                    switch (sqlmapSource.Type)
                    {
                        case SmartSqlMapSource.ResourceType.File:
                            {
                                LoadSmartSqlMap(config, sqlmapSource.Path);
                                break;
                            }
                        case SmartSqlMapSource.ResourceType.Directory:
                            {
                                var childSqlmapSources = Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sqlmapSource.Path), "*.xml");
                                foreach (var childSqlmapSource in childSqlmapSources)
                                {
                                    LoadSmartSqlMap(config, childSqlmapSource);
                                }
                                break;
                            }
                        default:
                            {
                                _logger.LogDebug($"LocalFileConfigLoader unknow SmartSqlMapSource.ResourceType:{sqlmapSource.Type}.");
                                break;
                            }
                    }
                }
            }
            _logger.LogDebug($"LocalFileConfigLoader Load: { _smartSqlOptions.SqlMapperPath} End");
            smartSqlMapper.(config);

            if (config.Settings.IsWatchConfigFile)
            {
                _logger.LogDebug($"LocalFileConfigLoader Load Add WatchConfig: { _smartSqlOptions.SqlMapperPath} Starting.");
                WatchConfig(smartSqlMapper);
                _logger.LogDebug($"LocalFileConfigLoader Load Add WatchConfig: { _smartSqlOptions.SqlMapperPath} End.");
            }
            return config;
        }

        private void LoadSmartSqlMap(SmartSqlMapConfig config, String sqlmapSourcePath)
        {
            _logger.LogDebug($"LoadSmartSqlMap Load: {sqlmapSourcePath}");
            var sqlmapStream = LoadConfigStream(sqlmapSourcePath);
            var sqlmap = LoadSmartSqlMap(sqlmapStream, config);
            config.SmartSqlMaps.Add(sqlmap);
        }

        public ConfigStream LoadConfigStream(string path)
        {
            var configStream = new ConfigStream
            {
                Path = path,
                Stream = FileLoader.Load(path)
            };
            return configStream;
        }

        private void LoadManifestSmartSqlMap(SmartSqlMapConfig config, string name)
        {
            var configStream = new ConfigStream
            {
                Path = name,
                Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)
            };
            var sqlmap = LoadSmartSqlMap(configStream, config);
            config.SmartSqlMaps.Add(sqlmap);
        }

        /// <summary>
        /// 监控配置文件-热更新
        /// </summary>
        /// <param name="smartSqlMapper"></param>
        /// <param name="config"></param>
        private void WatchConfig(ISmartSqlMapper smartSqlMapper)
        {
            var config = smartSqlMapper.SqlMapConfig;

            #region SmartSqlMapConfig File Watch

            _logger.LogDebug($"LocalFileConfigLoader Watch SmartSqlMapConfig: {config.Path} .");
            var cofigFileInfo = FileLoader.GetInfo(config.Path);
            FileWatcherLoader.Instance.Watch(cofigFileInfo, () =>
            {
                Thread.Sleep(DELAYED_LOAD_FILE);
                lock (this)
                {
                    try
                    {
                        _logger.LogDebug($"LocalFileConfigLoader Changed ReloadConfig: {config.Path} Starting");
                        var newConfig = Load(config.Path, smartSqlMapper);
                        _logger.LogDebug($"LocalFileConfigLoader Changed ReloadConfig: {config.Path} End");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                    }
                }
            });

            #endregion SmartSqlMapConfig File Watch

            #region SmartSqlMaps File Watch

            foreach (var sqlmap in config.SmartSqlMaps)
            {
                #region SqlMap File Watch

                _logger.LogDebug($"LocalFileConfigLoader Watch SmartSqlMap: {sqlmap.Path} .");
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.Path);
                FileWatcherLoader.Instance.Watch(sqlMapFileInfo, () =>
                {
                    Thread.Sleep(DELAYED_LOAD_FILE);
                    lock (this)
                    {
                        try
                        {
                            _logger.LogDebug($"LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} Starting");
                            var sqlmapStream = LoadConfigStream(sqlmap.Path);
                            var newSqlmap = LoadSmartSqlMap(sqlmapStream, config);
                            sqlmap.Scope = newSqlmap.Scope;
                            sqlmap.Statements = newSqlmap.Statements;
                            sqlmap.Caches = newSqlmap.Caches;
                            config.ResetMappedStatements();
                            smartSqlMapper.CacheManager.ResetMappedCaches();
                            _logger.LogDebug($"LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} End");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                        }
                    }
                });

                #endregion SqlMap File Watch
            }

            #endregion SmartSqlMaps File Watch
        }
    }
}