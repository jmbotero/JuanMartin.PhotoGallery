USE Photo_Gallery

DROP TABLE IF EXISTS `tblpasswordreset`;
CREATE TABLE IF NOT EXISTS `tblpasswordreset` (
  `user_id` int NOT NULL,
  `activation_code` varchar(36) NOT NULL DEFAULT '',
  `request_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`activation_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
