--
-- PostgreSQL database dump
--

-- Dumped from database version 16.3
-- Dumped by pg_dump version 16.3

-- Started on 2026-04-08 11:23:19

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 7 (class 2615 OID 299055)
-- Name: tenant; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA tenant;


ALTER SCHEMA tenant OWNER TO postgres;

--
-- TOC entry 10 (class 2615 OID 315436)
-- Name: tenant_string; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA tenant_string;


ALTER SCHEMA tenant_string OWNER TO postgres;

--
-- TOC entry 8 (class 2615 OID 307313)
-- Name: tenant_string11; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA tenant_string11;


ALTER SCHEMA tenant_string11 OWNER TO postgres;

--
-- TOC entry 9 (class 2615 OID 307458)
-- Name: tenant_test; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA tenant_test;


ALTER SCHEMA tenant_test OWNER TO postgres;

--
-- TOC entry 2 (class 3079 OID 290791)
-- Name: dblink; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS dblink WITH SCHEMA public;


--
-- TOC entry 5026 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION dblink; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION dblink IS 'connect to other PostgreSQL databases from within a database';


--
-- TOC entry 230 (class 1259 OID 307258)
-- Name: global_users_global_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.global_users_global_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.global_users_global_user_id_seq OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 223 (class 1259 OID 290884)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    role_id integer NOT NULL,
    role_name character varying(255)
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 290887)
-- Name: roles_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roles_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roles_role_id_seq OWNER TO postgres;

--
-- TOC entry 5028 (class 0 OID 0)
-- Dependencies: 224
-- Name: roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roles_role_id_seq OWNED BY public.roles.role_id;


--
-- TOC entry 232 (class 1259 OID 307276)
-- Name: tenants; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tenants (
    tenant_id integer NOT NULL,
    company_name character varying(255) NOT NULL,
    schema_name character varying(63) NOT NULL,
    is_active boolean DEFAULT true,
    created_on timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.tenants OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 307247)
-- Name: tenants_tenant_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tenants_tenant_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tenants_tenant_id_seq OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 307275)
-- Name: tenants_tenant_id_seq1; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tenants_tenant_id_seq1
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tenants_tenant_id_seq1 OWNER TO postgres;

--
-- TOC entry 5029 (class 0 OID 0)
-- Dependencies: 231
-- Name: tenants_tenant_id_seq1; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tenants_tenant_id_seq1 OWNED BY public.tenants.tenant_id;


--
-- TOC entry 221 (class 1259 OID 290868)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    first_name character varying(255) NOT NULL,
    last_name character varying(255) NOT NULL,
    username character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    phone character varying(15),
    address text,
    zipcode character varying(15),
    profile_picture character varying,
    role_id integer NOT NULL,
    created_on timestamp without time zone NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL,
    employment_start_date timestamp without time zone,
    date_of_birth timestamp without time zone,
    "position" character varying(100),
    is_two_factor_enabled boolean DEFAULT false NOT NULL,
    two_factor_secret text,
    two_factor_enabled_on timestamp without time zone,
    failed_login_count integer DEFAULT 0 NOT NULL,
    last_failed_login timestamp without time zone,
    lockout_until timestamp without time zone,
    tenant_id integer NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 290876)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 5031 (class 0 OID 0)
-- Dependencies: 222
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 226 (class 1259 OID 299006)
-- Name: departments; Type: TABLE; Schema: tenant; Owner: postgres
--

CREATE TABLE tenant.departments (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    manager_id integer
);


ALTER TABLE tenant.departments OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 299005)
-- Name: departments_id_seq; Type: SEQUENCE; Schema: tenant; Owner: postgres
--

CREATE SEQUENCE tenant.departments_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE tenant.departments_id_seq OWNER TO postgres;

--
-- TOC entry 5033 (class 0 OID 0)
-- Dependencies: 225
-- Name: departments_id_seq; Type: SEQUENCE OWNED BY; Schema: tenant; Owner: postgres
--

ALTER SEQUENCE tenant.departments_id_seq OWNED BY tenant.departments.id;


--
-- TOC entry 228 (class 1259 OID 299018)
-- Name: employee_list; Type: TABLE; Schema: tenant; Owner: postgres
--

CREATE TABLE tenant.employee_list (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    email character varying(100),
    department_id integer NOT NULL,
    salary numeric,
    created_on timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE tenant.employee_list OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 299017)
-- Name: employees_id_seq; Type: SEQUENCE; Schema: tenant; Owner: postgres
--

CREATE SEQUENCE tenant.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE tenant.employees_id_seq OWNER TO postgres;

--
-- TOC entry 5035 (class 0 OID 0)
-- Dependencies: 227
-- Name: employees_id_seq; Type: SEQUENCE OWNED BY; Schema: tenant; Owner: postgres
--

ALTER SEQUENCE tenant.employees_id_seq OWNED BY tenant.employee_list.id;


--
-- TOC entry 243 (class 1259 OID 315439)
-- Name: departments; Type: TABLE; Schema: tenant_string; Owner: postgres
--

CREATE TABLE tenant_string.departments (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    manager_id integer
);


ALTER TABLE tenant_string.departments OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 315438)
-- Name: departments_id_seq; Type: SEQUENCE; Schema: tenant_string; Owner: postgres
--

ALTER TABLE tenant_string.departments ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME tenant_string.departments_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 241 (class 1259 OID 315437)
-- Name: employees_id_seq; Type: SEQUENCE; Schema: tenant_string; Owner: postgres
--

CREATE SEQUENCE tenant_string.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE tenant_string.employees_id_seq OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 315444)
-- Name: employee_list; Type: TABLE; Schema: tenant_string; Owner: postgres
--

CREATE TABLE tenant_string.employee_list (
    id integer DEFAULT nextval('tenant_string.employees_id_seq'::regclass) NOT NULL,
    name character varying(100) NOT NULL,
    email character varying(100),
    department_id integer NOT NULL,
    salary numeric,
    created_on timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE tenant_string.employee_list OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 307316)
-- Name: departments; Type: TABLE; Schema: tenant_string11; Owner: postgres
--

CREATE TABLE tenant_string11.departments (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    manager_id integer
);


ALTER TABLE tenant_string11.departments OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 307315)
-- Name: departments_id_seq; Type: SEQUENCE; Schema: tenant_string11; Owner: postgres
--

ALTER TABLE tenant_string11.departments ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME tenant_string11.departments_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 233 (class 1259 OID 307314)
-- Name: employees_id_seq; Type: SEQUENCE; Schema: tenant_string11; Owner: postgres
--

CREATE SEQUENCE tenant_string11.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE tenant_string11.employees_id_seq OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 307321)
-- Name: employee_list; Type: TABLE; Schema: tenant_string11; Owner: postgres
--

CREATE TABLE tenant_string11.employee_list (
    id integer DEFAULT nextval('tenant_string11.employees_id_seq'::regclass) NOT NULL,
    name character varying(100) NOT NULL,
    email character varying(100),
    department_id integer NOT NULL,
    salary numeric,
    created_on timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE tenant_string11.employee_list OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 307461)
-- Name: departments; Type: TABLE; Schema: tenant_test; Owner: postgres
--

CREATE TABLE tenant_test.departments (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    manager_id integer
);


ALTER TABLE tenant_test.departments OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 307460)
-- Name: departments_id_seq; Type: SEQUENCE; Schema: tenant_test; Owner: postgres
--

ALTER TABLE tenant_test.departments ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME tenant_test.departments_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 237 (class 1259 OID 307459)
-- Name: employees_id_seq; Type: SEQUENCE; Schema: tenant_test; Owner: postgres
--

CREATE SEQUENCE tenant_test.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE tenant_test.employees_id_seq OWNER TO postgres;

--
-- TOC entry 240 (class 1259 OID 307466)
-- Name: employee_list; Type: TABLE; Schema: tenant_test; Owner: postgres
--

CREATE TABLE tenant_test.employee_list (
    id integer DEFAULT nextval('tenant_test.employees_id_seq'::regclass) NOT NULL,
    name character varying(100) NOT NULL,
    email character varying(100),
    department_id integer NOT NULL,
    salary numeric,
    created_on timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE tenant_test.employee_list OWNER TO postgres;

--
-- TOC entry 4796 (class 2604 OID 290888)
-- Name: roles role_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles ALTER COLUMN role_id SET DEFAULT nextval('public.roles_role_id_seq'::regclass);


--
-- TOC entry 4800 (class 2604 OID 307279)
-- Name: tenants tenant_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tenants ALTER COLUMN tenant_id SET DEFAULT nextval('public.tenants_tenant_id_seq1'::regclass);


--
-- TOC entry 4792 (class 2604 OID 290891)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 4797 (class 2604 OID 299009)
-- Name: departments id; Type: DEFAULT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.departments ALTER COLUMN id SET DEFAULT nextval('tenant.departments_id_seq'::regclass);


--
-- TOC entry 4798 (class 2604 OID 299021)
-- Name: employee_list id; Type: DEFAULT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.employee_list ALTER COLUMN id SET DEFAULT nextval('tenant.employees_id_seq'::regclass);


--
-- TOC entry 4997 (class 0 OID 290884)
-- Dependencies: 223
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roles (role_id, role_name) FROM stdin;
1	Admin
3	SuperAdmin
2	User
\.


--
-- TOC entry 5006 (class 0 OID 307276)
-- Dependencies: 232
-- Data for Name: tenants; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tenants (tenant_id, company_name, schema_name, is_active, created_on) FROM stdin;
10	string	tenant_string	t	2026-03-27 12:36:19.518455
2	Globex IT	tenant_globex	t	2026-03-11 10:09:48.186554
1	Acme Corp	tenant	t	2026-03-11 10:09:48.186554
\.


--
-- TOC entry 4995 (class 0 OID 290868)
-- Dependencies: 221
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, first_name, last_name, username, email, password, phone, address, zipcode, profile_picture, role_id, created_on, is_deleted, employment_start_date, date_of_birth, "position", is_two_factor_enabled, two_factor_secret, two_factor_enabled_on, failed_login_count, last_failed_login, lockout_until, tenant_id) FROM stdin;
24	string	string	string	string@gmail.com	$2b$10$ncBsNS11syAXE/E/EFnWHeSMKuuGeGDL1eIW01KxXlXy5I.kirf6y	\N	\N	\N	\N	1	2026-03-27 12:36:20.257118	f	\N	\N	\N	f	\N	\N	0	\N	\N	10
1	devisha	gajjar	devisha123	admin@gmail.com	$2b$10$SZCKwlF2bxNrFyMSVwwOPem.yFo.wLTbCEu3cqFdUhCA4Sw9iAX6q	9106074255	some address	365520	\N	1	2025-06-12 18:38:48.877887	f	2023-01-01 00:00:00	1995-05-10 00:00:00	Admin	f	UK54IAOPHBKZ5MZOXHSHUG7KFDVJYYLQ	\N	0	\N	\N	1
3	Bhumi	Shah	bhumi.workid@gmail.com	bhumi.workid@gmail.com		\N	\N	\N	https://lh3.googleusercontent.com/a/ACg8ocIW_sifdVU3M5m2-V7y7fqinuOYzAY0c2oPo2wbqLtDS1RfRw=s96-c	2	2025-06-24 09:47:43.489574	f	2023-03-01 00:00:00	1994-02-18 00:00:00	Developer	f	\N	\N	0	\N	\N	1
23	superAdmin	superAdmin	superAdmin	superadmin@gmail.com	$2b$10$SZCKwlF2bxNrFyMSVwwOPem.yFo.wLTbCEu3cqFdUhCA4Sw9iAX6q	\N	\N	\N	\N	3	2026-03-17 11:21:14.290638	f	\N	\N	\N	f	RTQQ2YKWVNY4MW3VI4YOLI4TBMSRSHFS	\N	0	\N	\N	1
8	devisha	gajjar	devisha_5	devisha5@example.com	$2b$10$szckwlF2bxNrFyMSVwwOPem.yFo.wLTbCEu3cqFdUhCA4Sw9iAX6q	9106074228	some address	36400	\N	2	2025-11-24 14:25:12	f	2025-11-24 00:00:00	1997-08-25 00:00:00	Intern	f	\N	\N	0	\N	\N	1
9	user	trial	user123	user@gmail.com	$2b$10$sFziK7ELvBzoaTyJSVLMnOx8TUnI1U7Xf4pkJjI3Qh9I5x4XyVQEa	9106074228	abcd	364005	\N	2	2025-12-09 13:39:10.771062	f	\N	\N	\N	f	\N	\N	0	\N	\N	1
10	user	gajjar	test123	test@gmail.com	$2b$10$SqP9dfCs1I4Sz18aH3BMFOEP/nF2atGK6GXCJgurpTEE2YDeGtSgm	9106074288	fgrewtgw	360005	\N	2	2025-12-18 16:44:39.326463	f	\N	\N	\N	f	\N	\N	0	\N	\N	1
13	Devisha	Gajjar	devishagajjar68@gmail.com	devishagajjar68@gmail.com		\N	\N	\N	https://lh3.googleusercontent.com/a/ACg8ocKJjBfpnCGDtNxeJpi6Gkl2FaQkCoMebE0DLs1_Bt8lQEINDw=s96-c	2	2025-12-31 12:00:19.503595	f	\N	\N	\N	f	\N	\N	0	\N	\N	1
4	manager	manager	manager12	manager@gmail.com	$2b$10$SZCKwlF2bxNrFyMSVwwOPem.yFo.wLTbCEu3cqFdUhCA4Sw9iAX6q	9106074223\n	abcd\n	360001	\N	1	2025-06-12 18:38:48.877887	f	\N	\N	\N	t	VN67HFXLBON6CFAMSNP4DV2TQNZ7FHPI	\N	0	\N	\N	1
2	DEVISHA	GAJJAR	21ce153.devisha.gajjar@vvpedulink.ac.in	21ce153.devisha.gajjar@vvpedulink.ac.in	$2b$10$sFziK7ELvBzoaTyJSVLMnOx8TUnI1U7Xf4pkJjI3Qh9I5x4XyVQEa	\N	\N	\N	https://lh3.googleusercontent.com/a/ACg8ocKcANtbNo8X5QRt3GcTuswDJcy5yvWubG1d2xgu-GZ6GE8_soY=s96-c	2	2025-06-23 17:18:39.820518	f	2023-01-15 00:00:00	1996-07-12 00:00:00	Manager	f	\N	\N	0	\N	\N	1
14	abc	abc	abc	abc@mailinator.com	$2b$10$KEESdsyCpb0.kIFCfkHjG.FTJ4F5AQImIiVDc5SapHiO1MDKTk3Fe	919106074286	fsef	265815	\N	2	2026-01-20 11:25:57.644763	f	\N	\N	\N	f	\N	\N	0	\N	\N	1
5	devisha	gajjar	devisha12\n	devisha@mailinator.com	$2b$10$K9lxMzP.IuEHraZVk6GTHulswn2AMYpbv2uE3UoA0ELlgz1kZKstW	9106074228	abcd	36400	\N	2	2025-11-24 14:25:12.86968	f	2025-05-10 00:00:00	1997-08-25 00:00:00	Intern	f	\N	\N	0	\N	\N	1
15	abcabc	snv	dkmvn	trial@gmail.com	$2b$10$otWVn0MQTYNvz0VxgLoZWe3e4KUaqStY24E6YdAeozx7ac5SF0WpS	919106074884			\N	2	2026-02-23 11:23:14.570452	f	\N	\N	\N	f	\N	\N	0	\N	\N	1
\.


--
-- TOC entry 5000 (class 0 OID 299006)
-- Dependencies: 226
-- Data for Name: departments; Type: TABLE DATA; Schema: tenant; Owner: postgres
--

COPY tenant.departments (id, name, manager_id) FROM stdin;
1	Human Resources	\N
2	Information Technology	\N
3	Finance	\N
4	Sales	\N
\.


--
-- TOC entry 5002 (class 0 OID 299018)
-- Dependencies: 228
-- Data for Name: employee_list; Type: TABLE DATA; Schema: tenant; Owner: postgres
--

COPY tenant.employee_list (id, name, email, department_id, salary, created_on) FROM stdin;
7	Mike Ross	mike.ross@legal.com	2	48000.00	2026-03-10 10:47:31.634493
8	Rachel Zane	rachel.zane@hr.com	2	52000.00	2026-03-10 10:47:31.634493
9	Harvey Specter	harvey.s@suits.com	3	95000.00	2026-03-10 10:47:31.634493
6	Jane Smith doey	jane.smith@company.com	1	62000.5	2026-03-10 10:47:31.634493
11	trial	trial2@gmail.com	1	12500	2026-03-19 11:23:01.829556
12	testing	testing@gmail.com	3	25000	2026-03-24 09:56:22.270176
13	1	1@gmail.com	1	25000	2026-03-24 09:59:21.667851
26	isha	isha@mailinator.com	3	125000	2026-03-31 10:25:26.671552
5	devisha	devisha@mailinator.com	2	125000	2026-03-24 14:12:29.853903
\.


--
-- TOC entry 5017 (class 0 OID 315439)
-- Dependencies: 243
-- Data for Name: departments; Type: TABLE DATA; Schema: tenant_string; Owner: postgres
--

COPY tenant_string.departments (id, name, manager_id) FROM stdin;
\.


--
-- TOC entry 5018 (class 0 OID 315444)
-- Dependencies: 244
-- Data for Name: employee_list; Type: TABLE DATA; Schema: tenant_string; Owner: postgres
--

COPY tenant_string.employee_list (id, name, email, department_id, salary, created_on) FROM stdin;
\.


--
-- TOC entry 5009 (class 0 OID 307316)
-- Dependencies: 235
-- Data for Name: departments; Type: TABLE DATA; Schema: tenant_string11; Owner: postgres
--

COPY tenant_string11.departments (id, name, manager_id) FROM stdin;
\.


--
-- TOC entry 5010 (class 0 OID 307321)
-- Dependencies: 236
-- Data for Name: employee_list; Type: TABLE DATA; Schema: tenant_string11; Owner: postgres
--

COPY tenant_string11.employee_list (id, name, email, department_id, salary, created_on) FROM stdin;
\.


--
-- TOC entry 5013 (class 0 OID 307461)
-- Dependencies: 239
-- Data for Name: departments; Type: TABLE DATA; Schema: tenant_test; Owner: postgres
--

COPY tenant_test.departments (id, name, manager_id) FROM stdin;
\.


--
-- TOC entry 5014 (class 0 OID 307466)
-- Dependencies: 240
-- Data for Name: employee_list; Type: TABLE DATA; Schema: tenant_test; Owner: postgres
--

COPY tenant_test.employee_list (id, name, email, department_id, salary, created_on) FROM stdin;
\.


--
-- TOC entry 5036 (class 0 OID 0)
-- Dependencies: 230
-- Name: global_users_global_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.global_users_global_user_id_seq', 1, false);


--
-- TOC entry 5037 (class 0 OID 0)
-- Dependencies: 224
-- Name: roles_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_role_id_seq', 3, true);


--
-- TOC entry 5038 (class 0 OID 0)
-- Dependencies: 229
-- Name: tenants_tenant_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tenants_tenant_id_seq', 1, false);


--
-- TOC entry 5039 (class 0 OID 0)
-- Dependencies: 231
-- Name: tenants_tenant_id_seq1; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tenants_tenant_id_seq1', 10, true);


--
-- TOC entry 5040 (class 0 OID 0)
-- Dependencies: 222
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_user_id_seq', 24, true);


--
-- TOC entry 5041 (class 0 OID 0)
-- Dependencies: 225
-- Name: departments_id_seq; Type: SEQUENCE SET; Schema: tenant; Owner: postgres
--

SELECT pg_catalog.setval('tenant.departments_id_seq', 4, true);


--
-- TOC entry 5042 (class 0 OID 0)
-- Dependencies: 227
-- Name: employees_id_seq; Type: SEQUENCE SET; Schema: tenant; Owner: postgres
--

SELECT pg_catalog.setval('tenant.employees_id_seq', 26, true);


--
-- TOC entry 5043 (class 0 OID 0)
-- Dependencies: 242
-- Name: departments_id_seq; Type: SEQUENCE SET; Schema: tenant_string; Owner: postgres
--

SELECT pg_catalog.setval('tenant_string.departments_id_seq', 1, false);


--
-- TOC entry 5044 (class 0 OID 0)
-- Dependencies: 241
-- Name: employees_id_seq; Type: SEQUENCE SET; Schema: tenant_string; Owner: postgres
--

SELECT pg_catalog.setval('tenant_string.employees_id_seq', 1, false);


--
-- TOC entry 5045 (class 0 OID 0)
-- Dependencies: 234
-- Name: departments_id_seq; Type: SEQUENCE SET; Schema: tenant_string11; Owner: postgres
--

SELECT pg_catalog.setval('tenant_string11.departments_id_seq', 1, false);


--
-- TOC entry 5046 (class 0 OID 0)
-- Dependencies: 233
-- Name: employees_id_seq; Type: SEQUENCE SET; Schema: tenant_string11; Owner: postgres
--

SELECT pg_catalog.setval('tenant_string11.employees_id_seq', 1, false);


--
-- TOC entry 5047 (class 0 OID 0)
-- Dependencies: 238
-- Name: departments_id_seq; Type: SEQUENCE SET; Schema: tenant_test; Owner: postgres
--

SELECT pg_catalog.setval('tenant_test.departments_id_seq', 1, false);


--
-- TOC entry 5048 (class 0 OID 0)
-- Dependencies: 237
-- Name: employees_id_seq; Type: SEQUENCE SET; Schema: tenant_test; Owner: postgres
--

SELECT pg_catalog.setval('tenant_test.employees_id_seq', 1, false);


--
-- TOC entry 4816 (class 2606 OID 290890)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 4824 (class 2606 OID 307283)
-- Name: tenants tenants_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tenants
    ADD CONSTRAINT tenants_pkey PRIMARY KEY (tenant_id);


--
-- TOC entry 4826 (class 2606 OID 307285)
-- Name: tenants tenants_schema_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tenants
    ADD CONSTRAINT tenants_schema_name_key UNIQUE (schema_name);


--
-- TOC entry 4810 (class 2606 OID 290879)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 4812 (class 2606 OID 290881)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4814 (class 2606 OID 290883)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 4818 (class 2606 OID 299011)
-- Name: departments departments_pkey; Type: CONSTRAINT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.departments
    ADD CONSTRAINT departments_pkey PRIMARY KEY (id);


--
-- TOC entry 4820 (class 2606 OID 299028)
-- Name: employee_list employees_email_key; Type: CONSTRAINT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.employee_list
    ADD CONSTRAINT employees_email_key UNIQUE (email);


--
-- TOC entry 4822 (class 2606 OID 299026)
-- Name: employee_list employees_pkey; Type: CONSTRAINT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.employee_list
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- TOC entry 4840 (class 2606 OID 315443)
-- Name: departments departments_pkey; Type: CONSTRAINT; Schema: tenant_string; Owner: postgres
--

ALTER TABLE ONLY tenant_string.departments
    ADD CONSTRAINT departments_pkey PRIMARY KEY (id);


--
-- TOC entry 4844 (class 2606 OID 315452)
-- Name: employee_list employees_pkey; Type: CONSTRAINT; Schema: tenant_string; Owner: postgres
--

ALTER TABLE ONLY tenant_string.employee_list
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- TOC entry 4828 (class 2606 OID 307320)
-- Name: departments departments_pkey; Type: CONSTRAINT; Schema: tenant_string11; Owner: postgres
--

ALTER TABLE ONLY tenant_string11.departments
    ADD CONSTRAINT departments_pkey PRIMARY KEY (id);


--
-- TOC entry 4832 (class 2606 OID 307329)
-- Name: employee_list employees_pkey; Type: CONSTRAINT; Schema: tenant_string11; Owner: postgres
--

ALTER TABLE ONLY tenant_string11.employee_list
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- TOC entry 4834 (class 2606 OID 307465)
-- Name: departments departments_pkey; Type: CONSTRAINT; Schema: tenant_test; Owner: postgres
--

ALTER TABLE ONLY tenant_test.departments
    ADD CONSTRAINT departments_pkey PRIMARY KEY (id);


--
-- TOC entry 4838 (class 2606 OID 307474)
-- Name: employee_list employees_pkey; Type: CONSTRAINT; Schema: tenant_test; Owner: postgres
--

ALTER TABLE ONLY tenant_test.employee_list
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- TOC entry 4841 (class 1259 OID 315459)
-- Name: IX_employee_list_department_id; Type: INDEX; Schema: tenant_string; Owner: postgres
--

CREATE INDEX "IX_employee_list_department_id" ON tenant_string.employee_list USING btree (department_id);


--
-- TOC entry 4842 (class 1259 OID 315458)
-- Name: employees_email_key; Type: INDEX; Schema: tenant_string; Owner: postgres
--

CREATE UNIQUE INDEX employees_email_key ON tenant_string.employee_list USING btree (email);


--
-- TOC entry 4829 (class 1259 OID 307336)
-- Name: IX_employee_list_department_id; Type: INDEX; Schema: tenant_string11; Owner: postgres
--

CREATE INDEX "IX_employee_list_department_id" ON tenant_string11.employee_list USING btree (department_id);


--
-- TOC entry 4830 (class 1259 OID 307335)
-- Name: employees_email_key; Type: INDEX; Schema: tenant_string11; Owner: postgres
--

CREATE UNIQUE INDEX employees_email_key ON tenant_string11.employee_list USING btree (email);


--
-- TOC entry 4835 (class 1259 OID 307481)
-- Name: IX_employee_list_department_id; Type: INDEX; Schema: tenant_test; Owner: postgres
--

CREATE INDEX "IX_employee_list_department_id" ON tenant_test.employee_list USING btree (department_id);


--
-- TOC entry 4836 (class 1259 OID 307480)
-- Name: employees_email_key; Type: INDEX; Schema: tenant_test; Owner: postgres
--

CREATE UNIQUE INDEX employees_email_key ON tenant_test.employee_list USING btree (email);


--
-- TOC entry 4845 (class 2606 OID 290892)
-- Name: users fk_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT fk_role FOREIGN KEY (role_id) REFERENCES public.roles(role_id);


--
-- TOC entry 4846 (class 2606 OID 307286)
-- Name: users fk_tenant; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT fk_tenant FOREIGN KEY (tenant_id) REFERENCES public.tenants(tenant_id) ON DELETE CASCADE;


--
-- TOC entry 4847 (class 2606 OID 299012)
-- Name: departments departments_manager_id_fkey; Type: FK CONSTRAINT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.departments
    ADD CONSTRAINT departments_manager_id_fkey FOREIGN KEY (manager_id) REFERENCES public.users(user_id);


--
-- TOC entry 4848 (class 2606 OID 299029)
-- Name: employee_list employees_department_id_fkey; Type: FK CONSTRAINT; Schema: tenant; Owner: postgres
--

ALTER TABLE ONLY tenant.employee_list
    ADD CONSTRAINT employees_department_id_fkey FOREIGN KEY (department_id) REFERENCES tenant.departments(id) ON DELETE CASCADE;


--
-- TOC entry 4851 (class 2606 OID 315453)
-- Name: employee_list employees_department_id_fkey; Type: FK CONSTRAINT; Schema: tenant_string; Owner: postgres
--

ALTER TABLE ONLY tenant_string.employee_list
    ADD CONSTRAINT employees_department_id_fkey FOREIGN KEY (department_id) REFERENCES tenant_string.departments(id) ON DELETE CASCADE;


--
-- TOC entry 4849 (class 2606 OID 307330)
-- Name: employee_list employees_department_id_fkey; Type: FK CONSTRAINT; Schema: tenant_string11; Owner: postgres
--

ALTER TABLE ONLY tenant_string11.employee_list
    ADD CONSTRAINT employees_department_id_fkey FOREIGN KEY (department_id) REFERENCES tenant_string11.departments(id) ON DELETE CASCADE;


--
-- TOC entry 4850 (class 2606 OID 307475)
-- Name: employee_list employees_department_id_fkey; Type: FK CONSTRAINT; Schema: tenant_test; Owner: postgres
--

ALTER TABLE ONLY tenant_test.employee_list
    ADD CONSTRAINT employees_department_id_fkey FOREIGN KEY (department_id) REFERENCES tenant_test.departments(id) ON DELETE CASCADE;


--
-- TOC entry 5024 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: pg_database_owner
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


--
-- TOC entry 5025 (class 0 OID 0)
-- Dependencies: 7
-- Name: SCHEMA tenant; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA tenant TO empmgmt_app;
GRANT USAGE ON SCHEMA tenant TO tenant1_role;


--
-- TOC entry 5027 (class 0 OID 0)
-- Dependencies: 223
-- Name: TABLE roles; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.roles TO empmgmt_app;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.roles TO tenant1_role;


--
-- TOC entry 5030 (class 0 OID 0)
-- Dependencies: 221
-- Name: TABLE users; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.users TO empmgmt_app;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.users TO global_app_role;


--
-- TOC entry 5032 (class 0 OID 0)
-- Dependencies: 226
-- Name: TABLE departments; Type: ACL; Schema: tenant; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE tenant.departments TO empmgmt_app;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE tenant.departments TO tenant1_role;


--
-- TOC entry 5034 (class 0 OID 0)
-- Dependencies: 228
-- Name: TABLE employee_list; Type: ACL; Schema: tenant; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE tenant.employee_list TO empmgmt_app;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE tenant.employee_list TO tenant1_role;


--
-- TOC entry 2141 (class 826 OID 299056)
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: tenant; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA tenant GRANT SELECT,INSERT,DELETE,UPDATE ON TABLES TO empmgmt_app;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA tenant GRANT SELECT,INSERT,DELETE,UPDATE ON TABLES TO tenant1_role;


-- Completed on 2026-04-08 11:23:19

--
-- PostgreSQL database dump complete
--

