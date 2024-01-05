USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspUpdateRanking`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspUpdateRanking` (IN `UserID` INT, IN `PhotographyID` BIGINT, IN `UserRank` INT)   
BEGIN
	DECLARE id INT;

	SET id=-1;
	
	SELECT *
	FROM tblUser u
 	WHERE u.id = UserID;

	IF FOUND_ROWS() THEN 	
		SELECT *
		FROM tblphotography p
	 	WHERE p.id = PhotographyID;

		IF FOUND_ROWS() THEN 	
				SELECT r.id
					INTO id
				FROM tblranking r
			 	WHERE r.photography_id = PhotographyID
				AND r.user_id = UserID;	
				
			IF NOT FOUND_ROWS() THEN 				 	
				INSERT INTO tblranking(user_id, photography_id, _rank) VALUE(UserID, PhotographyID, UserRank);
				SET id=LAST_INSERT_ID();
			ELSE
				UPDATE tblRanking
				SET _rank = UserRank
			 	WHERE photography_id = PhotographyID
				AND user_id = UserID;	
			END IF;
		END IF;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Set rank = ',UserRank, ' for (',PhotographyId,')'),'uspUpdateRanking',0);
	END IF;

	SELECT id;
END//
