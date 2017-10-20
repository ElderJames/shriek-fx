/*
Navicat PGSQL Data Transfer

Source Server         : postgres
Source Server Version : 100000
Source Host           : localhost:5432
Source Database       : shriek_eventstore
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 70300
File Encoding         : 65001

Date: 2017-10-17 19:27:59
*/


-- ----------------------------
-- Table structure for event_store
-- ----------------------------
DROP TABLE "event_store";
CREATE TABLE "event_store" (
"AggregateId" char(36),
"Data" text,
"MessageType" text,
"Version" int2,
"Timestamp" date,
"User" varchar(255)
)
WITHOUT OIDS 

;

-- ----------------------------
-- Records of event_store
-- ----------------------------
BEGIN;
COMMIT;

-- ----------------------------
-- Table structure for memento_store
-- ----------------------------
DROP TABLE "memento_store";
CREATE TABLE "memento_store" (
"AggregateId" char(36),
"Data" text,
"Timestamp" date,
"Version" int2
)
WITHOUT OIDS 

;

-- ----------------------------
-- Records of memento_store
-- ----------------------------
BEGIN;
COMMIT;
