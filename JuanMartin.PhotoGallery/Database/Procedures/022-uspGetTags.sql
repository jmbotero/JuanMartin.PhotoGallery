USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetTags`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetTags` (IN `CurrentPage` INT, IN `PageSize` INT)   
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT DISTINCT t.word AS  'Tag'
	FROM tbltag t
	LIMIT rec_take
	OFFSET rec_skip;
END//
