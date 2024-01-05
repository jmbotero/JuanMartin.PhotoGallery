USE Photo_Gallery

DROP TABLE IF EXISTS `tblaudit`;
CREATE TABLE IF NOT EXISTS `tblaudit` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `event_dtm` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `is_error` tinyint(1) NOT NULL DEFAULT '0',
  `message` varchar(250) NOT NULL,
  `_source` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=654 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
