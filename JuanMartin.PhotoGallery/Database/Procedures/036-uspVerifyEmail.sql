USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspVerifyEmail`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspVerifyEmail` (IN `Email` VARCHAR(100))   
BEGIN
	SELECT u.id AS 'Id', u.login AS 'Login'
	FROM tbluser u
	WHERE u.email = Email;
	
 	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS 'Id', '' AS 'Login';
	END IF;
END//
