USE Photo_Gallery

DROP TABLE IF EXISTS `tblorderitem`;
CREATE TABLE IF NOT EXISTS `tblorderitem` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `photography_id` bigint NOT NULL,
  `_index` int NOT NULL DEFAULT '1',
  `add_dtm` datetime NOT NULL,
  `update_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
