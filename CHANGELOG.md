# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [unreleased]

### [added] - 2026-1-16 - Zdeněk Relich

- Implementation of DAO pattern for manipulating information about bank accounts

### [added] - 2026-1-17 - Zdeněk Relich

- Implementation of Command pattern and P2P pattern

### [added] - 2026-1-20 - Zdeněk Relich

- Implementation of thread-safe Singleton pattern
- Added a TCP Server

### [edited] - 2026-1-23
- Commands now work asynchronously
- Application is now more configurable (server and node)

### [edited] - 2026-1-24
- Commands return messaages in english
- Application is now more configurable (server and node)

### [edited] - 2026-1-25
- bank_db.json is now created if it doesnt't exist