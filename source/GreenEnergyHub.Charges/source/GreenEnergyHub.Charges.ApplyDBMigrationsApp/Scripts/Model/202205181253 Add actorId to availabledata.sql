﻿------------------------------------------------------------------------------------------------------------------------
-- Add ActorId to AvailableData
-- AvailableChargeData, AvailableChargeReceiptData, AvailableChargeLinksData, AvailableChargeLinksReceiptData
------------------------------------------------------------------------------------------------------------------------
BEGIN TRANSACTION

-- Add foreign key field to table
ALTER TABLE [MessageHub].[AvailableChargeData]
    ADD ActorId [uniqueidentifier]
GO

-- Populate field, if any entries in current available table
UPDATE [MessageHub].[AvailableChargeData]
    SET ActorId =
        (SELECT Id FROM [Charges].[MarketParticipant] MP
        WHERE MP.MarketParticipantId = RecipientId AND MP.BusinessProcessRole = RecipientRole)
GO

-- Change field to be NOT NULL
ALTER TABLE [MessageHub].[AvailableChargeData]
    ALTER COLUMN ActorId uniqueidentifier NOT NULL;
GO

-- Add foreign key field to table
ALTER TABLE [MessageHub].[AvailableChargeReceiptData]
    ADD ActorId [uniqueidentifier]
GO

-- Populate field, if any entries in current available table
UPDATE [MessageHub].[AvailableChargeReceiptData]
    SET ActorId =
        (SELECT Id FROM [Charges].[MarketParticipant] MP
        WHERE MP.MarketParticipantId = RecipientId AND MP.BusinessProcessRole = RecipientRole)
GO

-- Change field to be NOT NULL
ALTER TABLE [MessageHub].[AvailableChargeReceiptData]
    ALTER COLUMN ActorId uniqueidentifier NOT NULL;
GO

-- Add foreign key field to table
ALTER TABLE [MessageHub].[AvailableChargeLinksData]
    ADD ActorId [uniqueidentifier]
GO

-- Populate field, if any entries in current available table
UPDATE [MessageHub].[AvailableChargeLinksData]
    SET ActorId =
        (SELECT Id FROM [Charges].[MarketParticipant] MP
        WHERE MP.MarketParticipantId = RecipientId AND MP.BusinessProcessRole = RecipientRole)
GO

-- Change field to be NOT NULL
ALTER TABLE [MessageHub].[AvailableChargeLinksData]
    ALTER COLUMN ActorId uniqueidentifier NOT NULL;
GO

-- Add foreign key field to table
ALTER TABLE [MessageHub].[AvailableChargeLinksReceiptData]
    ADD ActorId [uniqueidentifier]
GO

-- Populate field, if any entries in current available table
UPDATE [MessageHub].[AvailableChargeLinksReceiptData]
    SET ActorId =
        (SELECT Id FROM [Charges].[MarketParticipant] MP
        WHERE MP.MarketParticipantId = RecipientId AND MP.BusinessProcessRole = RecipientRole)
GO

-- Change field to be NOT NULL
ALTER TABLE [MessageHub].[AvailableChargeLinksReceiptData]
    ALTER COLUMN ActorId uniqueidentifier NOT NULL;
GO

COMMIT TRANSACTION