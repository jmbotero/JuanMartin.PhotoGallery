USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetPageCount`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetPageCount` (IN `PageSize` INT)   
BEGIN
	DECLARE photoCount INT;
	DECLARE pageCount INT;
	
	SELECT COUNT(*)
	INTO photoCount
	FROM  vwphotographydetails;
	
	SET pageCount = (photoCount / PageSize) + 1;
	
	SELECT pageCount;
END//
