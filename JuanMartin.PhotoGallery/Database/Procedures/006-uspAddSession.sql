USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddSession`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddSession` (IN `UserID` INT)   
BEGIN
	DECLARE id INT;
	
	INSERT INTO tblSession(user_id, start_dtm) VALUE(UserID, CURRENT_TIMESTAMP());
	SET id=LAST_INSERT_ID();
	
	SELECT id;
END//
