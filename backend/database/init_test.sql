CREATE TABLE medicos (
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    crm VARCHAR(20) UNIQUE NOT NULL,
    especialidade VARCHAR(100) NOT NULL,
    senhahash TEXT NOT NULL,
    cidade VARCHAR(100) NOT NULL,
    uf VARCHAR(2) NOT NULL
);

CREATE TABLE pacientes (
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cpf VARCHAR(100) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    senhahash TEXT NOT NULL
);


CREATE TABLE horariosdisponiveis (
    id UUID PRIMARY KEY,
    medicoid UUID REFERENCES medicos(id) ON DELETE CASCADE,
    datahora TIMESTAMP NOT NULL,
    datahorafim TIMESTAMP NOT NULL,
    status VARCHAR(20) NOT NULL
);

CREATE TABLE consultas (
    id UUID PRIMARY KEY,
    pacienteid UUID REFERENCES pacientes(id) ON DELETE CASCADE,
    medicoid UUID REFERENCES medicos(id) ON DELETE CASCADE,
    horarioid UUID REFERENCES horariosdisponiveis(id) ON DELETE CASCADE,
    status VARCHAR(20),
    justificativacancelamento TEXT,
    criadoem TIMESTAMP,
    datarespostamedico TIMESTAMP,
    datacancelamento TIMESTAMP
);

DELETE FROM consultas;
DELETE FROM horariosdisponiveis;
DELETE FROM pacientes;
DELETE FROM medicos;


INSERT INTO medicos (id, nome, crm, especialidade, senhahash, cidade, uf)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'Médico Teste',
    '123456',
    'Clínico Geral',
    '$2a$11$EfIb7BgzMznwSc1BtWGBgOi8aMi/zqFvX7F/yaC4t20fGZ0GH8.q2',
    'Belo Horizonte',
    'MG'
);

INSERT INTO pacientes (id, nome, cpf, email, senhahash)
VALUES (
    '00000000-0000-0000-0000-000000000002',
    'Paciente Teste',
    '12345678901',
    'paciente@teste.com',
    '$2a$11$EfIb7BgzMznwSc1BtWGBgOi8aMi/zqFvX7F/yaC4t20fGZ0GH8.q2'
);

INSERT INTO horariosdisponiveis (id, medicoid, datahora, datahorafim, status)
VALUES 
    ('00000000-0000-0000-0000-000000000101', '00000000-0000-0000-0000-000000000001', '2025-04-20 10:00:00', '2025-04-20 10:30:00', 'Disponivel'),
    ('00000000-0000-0000-0000-000000000102', '00000000-0000-0000-0000-000000000001', '2025-04-20 11:00:00', '2025-04-20 11:30:00', 'Disponivel');

INSERT INTO consultas (
    id,
    pacienteid,
    medicoid,
    horarioid,
    status,
    justificativacancelamento,
    criadoem,
    datarespostamedico,
    datacancelamento
)
VALUES (
    '00000000-0000-0000-0000-000000000301', -- id da consulta
    '00000000-0000-0000-0000-000000000002', -- paciente de teste
    '00000000-0000-0000-0000-000000000001', -- médico de teste
    '00000000-0000-0000-0000-000000000101', -- horário disponível de teste
    'Confirmada',                           -- status
    NULL,                                   -- justificativaCancelamento
    '2025-04-20 08:00:00',                  -- criadoem
    NULL,                                   -- datarespostamedico
    NULL                                    -- datacancelamento
);
