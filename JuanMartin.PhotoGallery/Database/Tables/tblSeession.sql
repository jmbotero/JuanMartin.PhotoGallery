USE Photo_Gallery

DROP TABLE IF EXISTS `tblsession`;
CREATE TABLE IF NOT EXISTS `tblsession` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `start_dtm` datetime NOT NULL,
  `end_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
