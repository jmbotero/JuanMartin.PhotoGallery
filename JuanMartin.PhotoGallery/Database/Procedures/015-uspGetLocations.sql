USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetLocations`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetLocations` (IN `CurrentPage` INT, IN `PageSize` INT)   
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT DISTINCT l._reference AS 'Location'
	FROM tblLocation l
	LIMIT rec_take
	OFFSET rec_skip;
END//
