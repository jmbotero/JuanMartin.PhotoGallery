USE Photo_Gallery

DROP TABLE IF EXISTS `tblstate`;
CREATE TABLE IF NOT EXISTS `tblstate` (
  `remote_host` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `user_id` int NOT NULL DEFAULT '-1',
  `redirect_controller` varchar(50) NOT NULL DEFAULT '',
  `redirect_action` varchar(50) NOT NULL DEFAULT '',
  `redirect_route_id` int NOT NULL DEFAULT '0',
  `redirect_routevalues` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `event_dtm` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`remote_host`,`user_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
