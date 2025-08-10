FROM postgres:17

# Install PostGIS
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        postgresql-17-postgis-3 \
        postgresql-17-postgis-3-scripts \
    && rm -rf /var/lib/apt/lists/*

# Create extension script
COPY docker/scripts/init-postgis.sql /docker-entrypoint-initdb.d/
