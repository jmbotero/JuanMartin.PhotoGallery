USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspGetPhotographyIdsList`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspGetPhotographyIdsList` (IN `UserId` INT, IN `SetSource` INT, IN `SearchQuery` VARCHAR(150), IN `OrderId` INT)  NO SQL SQL SECURITY INVOKER 
BEGIN
	IF (SetSource = 0) THEN /* from all gallery */
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
	   FROM(
				SELECT v.*
				FROM  (SELECT @p1:=UserId p) parm , vwphotographywithranking v
				ORDER BY v.AverageRank DESC, v.Id) sub;
	ELSEIF (SetSource = 1) THEN /* from search query*/
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
		FROM 
			((
				SELECT v.*
				FROM (SELECT @p1:=UserId p) parm ,tblphotographytags pt 
				JOIN vwphotographywithranking v 
					ON v.id = pt.photography_id
				JOIN tbltag t 
					ON t.id = pt.tag_id 
				WHERE t.word REGEXP SearchQuery
				ORDER BY v.AverageRank DESC, v.Id)
			UNION
			(
				SELECT v.*
				FROM vwphotographywithranking v 
				WHERE v.Location REGEXP SearchQuery
				ORDER BY v.AverageRank DESC, v.Id
			))sub;
	ELSEIF (SetSource = 2) THEN /* from order*/
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
	   FROM(
				SELECT v.*
				FROM (SELECT @p1:=UserId p) parm ,tblorderitem o 
				JOIN vwphotographywithranking v 
				ON v.id = o.photography_id
				WHERE o.order_id = OrderId
				ORDER BY o._index ASC
			) sub;
	END IF;
END//
