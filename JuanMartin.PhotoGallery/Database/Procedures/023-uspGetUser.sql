USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetUser`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetUser` (IN `UserName` VARCHAR(50), IN `UserPassword` VARCHAR(255))   
BEGIN
	SELECT u.id AS 'Id', u.login AS 'Login', u._password AS 'Password', u.email AS 'Email'
	FROM tbluser u
	WHERE u.login = UserName
	AND u._password = MD5(UserPassword);
	
 	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS 'Id', '' AS 'Login','' AS 'Password','' AS 'Email';
	END IF;
END//
