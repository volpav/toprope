Toprope
=======

Toprope is a search engine for rock climbers.

Setup
=====

Toprope runs on IIS and use SQL Server as a storage. To create the database tables, run all the scripts under the `/Data` folder (database name should be `Toprope`). 

To aggregate the content of the website from http://rockclimbing.com, run:

```
> Toprope.Aggregator.exe --parse rockclimbing.com
```

This will parse all the routes, sectors and areas (usually takes about 5-7 hours on my Internet connection). Notice that you can stop the aggregation manually by either killing the process or shutting down the machine - the aggregator will continue from the last checkpoint next time you run it.

After the above is done, run the following to dump the parsed data (residing under `C:\Toprope files') into the database:

```
> Toprope.Aggregator.exe --dump
```

And don't forget about the images (they'll need to be copied manually into `/Content/Images` from the parse location).

