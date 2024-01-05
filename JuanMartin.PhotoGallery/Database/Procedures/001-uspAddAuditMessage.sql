USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddAuditMessage`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddAuditMessage` (IN `UserID` INT, IN `Message` VARCHAR(250), IN `_Source` VARCHAR(100), IN `IsError` TINYINT(1))   
BEGIN
	INSERT INTO tblaudit(user_id, event_dtm, message, is_error,_source) VALUES(UserID, CURRENT_TIMESTAMP(), Message, IsError,_Source);
END//																							