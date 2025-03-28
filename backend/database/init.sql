-- Tabela de Médicos
CREATE TABLE Medicos (
    Id UUID PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    CRM VARCHAR(20) UNIQUE NOT NULL,
    Especialidade VARCHAR(100) NOT NULL
);

-- Tabela de Pacientes
CREATE TABLE Pacientes (
    Id UUID PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    CPF VARCHAR(11) UNIQUE NOT NULL
);

-- Tabela de Horários Disponíveis
CREATE TABLE HorariosDisponiveis (
    Id UUID PRIMARY KEY,
    MedicoId UUID NOT NULL REFERENCES Medicos(Id),
    DataHora TIMESTAMP NOT NULL,
    Status VARCHAR(20) DEFAULT 'livre'
);

-- Tabela de Consultas
CREATE TABLE Consultas (
    Id UUID PRIMARY KEY,
    PacienteId UUID NOT NULL REFERENCES Pacientes(Id),
    MedicoId UUID NOT NULL REFERENCES Medicos(Id),
    HorarioId UUID NOT NULL REFERENCES HorariosDisponiveis(Id),
    Status VARCHAR(20) DEFAULT 'agendada',
    JustificativaCancelamento TEXT,
    CriadoEm TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
