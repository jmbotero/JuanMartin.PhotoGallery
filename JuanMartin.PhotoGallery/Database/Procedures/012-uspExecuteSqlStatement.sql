USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspExecuteSqlStatement`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspExecuteSqlStatement` (IN `Statement` VARCHAR(500))   
BEGIN
	SET @qry = Statement;
	
	PREPARE stmt FROM @qry;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
END//
