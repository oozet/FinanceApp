CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS persons (
    person_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    email VARCHAR(100) UNIQUE NOT NULL, 
);

CREATE TYPE transaction_type AS ENUM ('Deposit', 'Withdrawal');

CREATE TABLE IF NOT EXISTS transactions (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    amount_minor_unit BIGINT NOT NULL,
    account_number INT NOT NULL REFERENCES accounts(account_number),
    type transaction_type NOT NULL,
    created_at TIMESTAMP DEFAULT current_timestamp NOT NULL,
    deleted_at TIMESTAMP DEFAULT null
);

CREATE TYPE account_type AS ENUM ('Private', 'Savings', 'Business');

CREATE TABLE IF NOT EXISTS accounts (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id),
    account_number INT UNIQUE NOT NULL,
    amount_minor_unit BIGINT DEFAULT 0 NOT NULL,
    type account_type DEFAULT 'private' NOT NULL,
    created_at TIMESTAMP DEFAULT current_timestamp NOT NULL,
    deleted_at TIMESTAMP DEFAULT NULL
);