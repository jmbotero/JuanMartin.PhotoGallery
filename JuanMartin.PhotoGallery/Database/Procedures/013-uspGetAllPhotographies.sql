USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetAllPhotographies`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetAllPhotographies` (IN `CurrentPage` INT, IN `PageSize` INT, IN `UserID` INT)   
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	IF(PageSize <> -1) THEN
		SET rec_take = PageSize;
		SET rec_skip = (CurrentPage - 1) * PageSize;
	
		SELECT v.* 
		FROM (SELECT @p1:=UserID p) parm , vwphotographywithranking v
		ORDER BY v.AverageRank DESC, v.Id DESC
		LIMIT rec_take
		OFFSET rec_skip;
	ELSE
		SELECT v.* 
		FROM (SELECT @p1:=UserID p) parm , vwphotographywithranking v
		ORDER BY v.AverageRank DESC, v.Id DESC;
	END IF;
END//
