USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetPhotographiesBySearch`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetPhotographiesBySearch` (IN `UserID` INT, IN `SearchQuery` VARCHAR(150), IN `CurrentPage` INT, IN `PageSize` INT)  NO SQL 
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	(SELECT DISTINCT v.*
	FROM (SELECT @p1:=UserID p) parm ,tblphotographytags pt 
	JOIN vwphotographywithranking v 
		ON v.id = pt.photography_id
	JOIN tbltag t 
		ON t.id = pt.tag_id 
	WHERE t.word REGEXP SearchQuery
	GROUP BY v.Id
	ORDER BY v.AverageRank DESC,v.Id DESC)
	UNION
	(SELECT DISTINCT v.*
	FROM vwphotographywithranking v 
	WHERE v.Location REGEXP SearchQuery
	GROUP BY v.Id
	ORDER BY v.AverageRank DESC,v.Id DESC)
	LIMIT rec_take
	OFFSET rec_skip;
END//
