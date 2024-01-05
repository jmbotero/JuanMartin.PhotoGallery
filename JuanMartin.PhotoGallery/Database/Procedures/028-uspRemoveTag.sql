USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspRemoveTag`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspRemoveTag` (IN `UserID` INT, IN `Word` VARCHAR(50), IN `PhotographyId` BIGINT)   
BEGIN
	DECLARE id INT;
	DECLARE linkExists INT;

	SET id=-1;

	IF(PhotographyId<>-1) THEN
		SELECT k.id
		INTO id
		FROM tbltag k
	 	WHERE k.word = Word;
	 	
		IF FOUND_ROWS() THEN 	
			DELETE k.*
			FROM tblphotographytags k
		 	WHERE k.tag_id=id
		 	AND k.photography_id = PhotographyId;
	 	
			SET linkExists = EXISTS(
				SELECT pk.tag_id
				FROM tblphotographytags pk
				JOIN tblTag t
				ON t.id = pk.tag_id
				WHERE t.word = Word
				AND pk.photography_id = PhotographyId);
			
			IF (linkExists = 0) THEN
				DELETE t.*
				FROM tblTag t
				WHERE t.word = Word;
			END IF;
		ELSE
			SET id = -1;
		END IF;
	END IF;	
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Remove tag ''',Word,'''  for (',PhotographyId,')'),'uspRemoveTag',0);
	END IF;

	SELECT id;
END//
