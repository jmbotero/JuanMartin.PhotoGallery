USE Photo_Gallery

--
-- Constraints for table `tblphotographytags`
--
ALTER TABLE `tblphotographytags`
  ADD CONSTRAINT `FK_keyword` FOREIGN KEY (`tag_id`) REFERENCES `tbltag` (`id`),
  ADD CONSTRAINT `FK_keyword_photography` FOREIGN KEY (`photography_id`) REFERENCES `tblphotography` (`id`);

--
-- Constraints for table `tblranking`
--
ALTER TABLE `tblranking`
  ADD CONSTRAINT `FK_ranking_photography` FOREIGN KEY (`photography_id`) REFERENCES `tblphotography` (`id`),
  ADD CONSTRAINT `FK_ranking_user` FOREIGN KEY (`user_id`) REFERENCES `tbluser` (`id`);


SET FOREIGN_KEY_CHECKS=1;
COMMIT;
