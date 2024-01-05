USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspStoreActivationCode`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspStoreActivationCode` (IN `UserID` INT, IN `ActivationCode` VARCHAR(36))   
BEGIN
	INSERT INTO tblPasswordReset(user_id ,activation_code,  request_dtm) VALUE(UserID, ActivationCode, CURRENT_TIMESTAMP());
END//
