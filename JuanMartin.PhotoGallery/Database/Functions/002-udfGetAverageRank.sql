USE Photo_Gallery

DELIMITER //

DROP FUNCTION IF EXISTS `udfGetAverageRank`//
CREATE DEFINER = 'root'@'localhost' FUNCTION `udfGetAverageRank` (`PhotographyID` BIGINT) RETURNS FLOAT READS SQL DATA SQL SECURITY INVOKER 
BEGIN
	DECLARE AverageRank float;

	SELECT AVG(r._rank)
	INTO AverageRank
	FROM tblRanking r
 	WHERE r.photography_id = PhotographyID;

	IF NOT FOUND_ROWS() THEN
		SET AverageRanK = 0; 				 	
	END IF;
	
	RETURN AverageRank;
END//
