USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspConnectUserAndRemoteHost`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspConnectUserAndRemoteHost` (IN `UserID` INT, IN `RemoteHost` VARCHAR(50))   
BEGIN
	IF (UserID = 1) THEN
      DELETE s.*
		FROM tblstate s 
		WHERE  s.remote_host = RemoteHost AND s.user_id = UserID;
	END IF;

	UPDATE tblState s
	SET s.user_id = UserID
	WHERE s.remote_host = RemoteHost
	AND s.user_id = -1;
END//

