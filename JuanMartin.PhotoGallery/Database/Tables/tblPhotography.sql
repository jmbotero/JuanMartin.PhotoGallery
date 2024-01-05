USE Photo_Gallery

DROP TABLE IF EXISTS `tblphotography`;
CREATE TABLE IF NOT EXISTS `tblphotography` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `_source` int NOT NULL DEFAULT '0',
  `_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '',
  `filename` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `location_id` int DEFAULT NULL,
  `archive` smallint NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE,
  KEY `FK_photography_locations` (`location_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2926 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
