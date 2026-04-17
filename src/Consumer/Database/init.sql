CREATE TABLE IF NOT EXISTS transaction_events (
    id              BIGSERIAL PRIMARY KEY,
    event_id        UUID NOT NULL UNIQUE,
    event_type      VARCHAR(50) NOT NULL,
    amount          DECIMAL(18, 2) NOT NULL,
    account_from    VARCHAR(50) NOT NULL,
    account_to      VARCHAR(50) NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'processed',
    kafka_partition INT NOT NULL,
    kafka_offset    BIGINT NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_transaction_events_event_id  ON transaction_events (event_id);
CREATE INDEX idx_transaction_events_status    ON transaction_events (status);
CREATE INDEX idx_transaction_events_created_at ON transaction_events (created_at);