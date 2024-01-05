USE Photo_Gallery

DROP TABLE IF EXISTS `tblphotographytags`;
CREATE TABLE IF NOT EXISTS `tblphotographytags` (
  `photography_id` bigint DEFAULT NULL,
  `tag_id` int DEFAULT NULL,
  KEY `FK_keyword_photography` (`photography_id`),
  KEY `FK_keyword` (`tag_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
