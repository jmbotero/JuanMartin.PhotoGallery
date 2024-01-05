USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddUser`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddUser` (IN `Login` VARCHAR(50), IN `UserPassword` VARCHAR(255), IN `Email` VARCHAR(100))   
BEGIN
	DECLARE id INT;
	
	SELECT u.id 
	INTO id
	FROM tblUser u
 	WHERE u.login = Login;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblUser(login, _password, email) VALUE(Login, MD5(UserPassword), Email);
		SET id=LAST_INSERT_ID();
	ELSE
		SET id=-2; /* user exists */
	END IF;
	
	SELECT id;
END//
