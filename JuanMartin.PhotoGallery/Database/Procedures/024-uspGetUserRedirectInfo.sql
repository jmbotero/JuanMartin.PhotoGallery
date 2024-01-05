USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetUserRedirectInfo`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetUserRedirectInfo` (IN `RemoteHost` VARCHAR(50), IN `UserID` INT)   
BEGIN
	SELECT 	s.remote_host AS 'RemoteHost',
				s.redirect_controller AS 'Controller',
	    		s.redirect_action AS 'Action',
	    		s.redirect_route_id AS 'RouteID',
	    		s.redirect_routevalues AS 'QueryString'
	FROM tblState s
	WHERE s.remote_host = RemoteHost
	AND s.user_id = UserID
	ORDER BY s.event_dtm DESC;
	 
	IF NOT FOUND_ROWS() THEN 	
		SELECT '' AS 'RemoteHost', '' AS 'Controller', '' AS 'Action', -1 AS 'RouteID', '' AS 'QueryString';
	END IF; 
END//
