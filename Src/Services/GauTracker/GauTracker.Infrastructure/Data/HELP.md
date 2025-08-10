## Add Migration
| Action | Command |
| --- | --- |
| Add-Migration | EntityFrameworkCore\Add-Migration XXX -Context GauTrackerContext -Project GauTracker.Infrastructure -StartupProject GauTracker.API -Verbose -o Data/Migrations |

## Remove Migration
| Action | Command |
| --- | --- |
| Remove-Migration | EntityFrameworkCore\Remove-Migration -Context GauTrackerContext -Project GauTracker.Infrastructure -StartupProject GauTracker.API -Verbose |

## Update Database
| Action | Command |
| --- | --- |
| Update-Database | EntityFrameworkCore\Update-Database -Context GauTrackerContext -Project GauTracker.Infrastructure -StartupProject GauTracker.API -Verbose |