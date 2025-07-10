## CHANGELOG

## [1.0.3] - 2025-07-10
- Organized print level.

## [1.0.2] - 2024-10-31
- Modified when noticeItem is detected as destroyed, it will be removed from condition.

## [1.0.1] - 2024-03-15
- Modified set #ROOTNAMESPACE# symbol in script templates.

## [1.0.0] - 2024-02-02
- Stabled.

## [0.0.5-preview] - 2023-09-22
- Added default constructor for Logger.

## [0.0.4-preview] - 2023-08-27
- Added Logger by LoggingSystem.

## [0.0.3-preview] - 2023-05-26
- Added NoticeItem can deregister specific condition id.
- Modified NoticeItem RenewNotice method params use NoticeInfo instead, also supports method chaining.
- Extended Notify with NoticeItem (NoticeManager).
- Optimized NoticeItem cache.

## [0.0.2-preview] - 2023-05-22
- Added ListSet.
- Added Notify for NoticeItem (can notify by notice item).
- Added RenewNotice for NoticeItem (If use value type data must renew).
- Modified RegisterNotice supports assign to renew NoticeItem.
- Optimized cache operation use ListSet instead.

## [0.0.1-preview] - 2023-05-21
- Added NoticeSystem.