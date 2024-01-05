USE Photo_Gallery

DELIMITER //

DROP PROCEDURE IF EXISTS `uspAddTag`//
CREATE DEFINER = 'root'@'localhost' PROCEDURE `uspAddTag` (IN `UserID` INT, IN `Word` VARCHAR(50), IN `PhotographyId` BIGINT)   
BEGIN
	DECLARE id INT;
	DECLARE imageExists INT;
	DECLARE linkExists INT;
	
	SET imageExists = EXISTS(
	            SELECT p.filename 
					FROM tblphotography p
				 	WHERE p.id = PhotographyId);

	SET id = -1;
					 
	IF (imageExists = 1) THEN 	
		SELECT k.id 
		INTO id
		FROM tbltag k
	 	WHERE k.word = Word;
		
		IF NOT FOUND_ROWS() THEN 	
			INSERT INTO tbltag(word) VALUE(LOWER(Word));
			SET id=LAST_INSERT_ID();
		END IF;
		
		IF(PhotographyId<>-1 AND id > 0) THEN
			SET linkExists = EXISTS(
					SELECT pk.tag_id
					FROM tblphotographytags pk
					WHERE pk.tag_id = id
					AND pk.photography_id = PhotographyId);
			
			IF (linkExists = 0) THEN 	
				SET FOREIGN_KEY_CHECKS = 0;
				INSERT INTO tblPhotographytags(tag_id, photography_id) VALUE(id, PhotographyId);
				SET FOREIGN_KEY_CHECKS = 1;
			ELSE
				SET id = -2;
			END IF;
		END IF;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Add tag ''',Word, ''' for (',PhotographyId,')'),'uspAddTag',0);
	END IF;
	SELECT id;
END//
