USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspUpdateUserPassword`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspUpdateUserPassword` (IN `UserID` INT, IN `Login` VARCHAR(50), IN `UserPassword` VARCHAR(255))   
BEGIN
	DECLARE id INT;
	DECLARE login VARCHAR(50);
	DECLARE email VARCHAR(255);
	
	SELECT u.id, u.login, u.email 
	INTO id, login, email
	FROM tblUser u
 	WHERE u.login = Login
 	AND u.id = UserID;
	
	IF FOUND_ROWS() THEN 	
		UPDATE tbluser u 
		SET u._password = MD5(UserPassword)
 	WHERE u.login = Login
 	AND u.id = UserID;

		SELECT id AS 'Id', login AS 'Login', '' AS 'Password', email AS 'Email';
	ELSE
		SELECT -1 AS 'Id', '' AS 'Login','' AS 'Password','' AS 'Email';
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Updated password for ''', login, ''' (', id ,')'),'uspUpdateUserPassword',0);
	END IF;

	SELECT id;
END//
