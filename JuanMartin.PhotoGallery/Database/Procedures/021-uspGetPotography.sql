USE Photo_Gallery

DELIMITER //
	
DROP PROCEDURE IF EXISTS `uspGetPotography`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetPotography` (IN `Id` BIGINT, IN `UserID` INT)   
BEGIN
	SELECT v.* 
	FROM (SELECT @p1:=UserID p) parm , vwphotographywithranking v
	WHERE v.Id = Id;
	
	
	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS Id;
	END IF;
END//
