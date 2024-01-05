USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspSetUserRedirectInfo`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspSetUserRedirectInfo` (IN `UserID` INT, IN `RemoteHost` VARCHAR(50), IN `Controller` VARCHAR(50), IN `ControllerAction` VARCHAR(50), IN `RouteID` INT, IN `QueryString` VARCHAR(100))   
BEGIN
	REPLACE INTO tblstate(remote_host, user_id, redirect_controller, redirect_action, redirect_route_id, redirect_routevalues, event_dtm) VALUES(RemoteHost, UserID, Controller, ControllerAction, RouteID, QueryString, CURRENT_TIMESTAMP());
END//
