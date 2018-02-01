# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Added support for macOS

### Changed
- Updated PCL libs to .NET Standard 1.0 libs

### Removed
- Removed support for Windows Phone and WinRT

## [2.1.0] - 2017-03-12

### Added
- [iOS] Added support for iOS 10+ (UserNotifications framework) 

### Fixed
- [iOS] Fixed bug with notification title display for iOS 9 and older

## [2.0.2] - 2015-12-07

### Changed
- [Android] Added auto cancel when tapped

### Fixed
- [Android] Fixed issue with serializing scheduled notifications

## [2.0.1] - 2015-12-01

### Changed
- [Android] Updated to launch main Activity
- [Android] Updated to use `NotificationCompat` and `NotificationManagerCompat`

## [2.0.0] - 2015-12-01

### Added
- Added ability to schedule and cancel notifications

## [1.0.2] - 2015-05-27

### Fixed
- [iOS] Fixed issue with `NSDate` casting

## [1.0.1] - 2014-12-30

### Added
- Added documentation

## [1.0.0] - 2014-12-30

### Added
- Initial release
