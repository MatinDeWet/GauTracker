# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability in GauTracker, please report it
privately rather than opening a public issue.

- Use GitHub's [private vulnerability reporting](https://github.com/MatinDeWet/GauTracker/security/advisories/new)
  ("Report a vulnerability" under the **Security** tab), or
- Contact the maintainer directly.

Please include steps to reproduce, affected components, and any relevant logs.
We aim to acknowledge reports within a few days.

## Supported Versions

This project is under active development; security fixes are applied to the
`main` branch.

## Notes

- The PostgreSQL credentials in `appsettings.json` and `compose.yaml` are
  **local-development defaults only** and must never be reused in any shared or
  production environment. Supply real secrets via environment variables
  (`ConnectionStrings__GauDB`) or a secrets manager.
