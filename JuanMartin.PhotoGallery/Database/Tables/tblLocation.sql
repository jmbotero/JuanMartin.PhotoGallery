USE Photo_Gallery

DROP TABLE IF EXISTS `tbllocation`;
CREATE TABLE IF NOT EXISTS `tbllocation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ddd` float DEFAULT NULL,
  `_reference` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
