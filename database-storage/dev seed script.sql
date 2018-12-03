DROP TABLE post_tags;
DROP TABLE tags;
DROP TABLE post_header;
DROP TABLE post_body;

-- Create table structure
CREATE TABLE tags (
	id INT GENERATED ALWAYS AS IDENTITY,
	tag_text VARCHAR(64) NOT NULL,
	PRIMARY KEY (id),
	UNIQUE(tag_text)
);

CREATE TABLE post_body (
    id INT GENERATED ALWAYS AS IDENTITY,
    post_markdown VARCHAR NOT NULL,
	PRIMARY KEY (id)
);

CREATE TABLE post_header (
    id INT GENERATED ALWAYS AS IDENTITY,
	post_body_id INT NOT NULL REFERENCES post_body(id),
    post_title VARCHAR(254) NOT NULL,
	PRIMARY KEY (id),
	UNIQUE(post_title),
	UNIQUE(post_body_id)
);

CREATE TABLE post_tags (
	tag_id INT NOT NULL REFERENCES tags(id),
	post_header_id INT NOT NULL REFERENCES post_header(id),
	PRIMARY KEY(tag_id, post_header_id)
);

-- seed with test data
INSERT INTO tags (tag_text) VALUES ('test'), ('test2'), ('example');

INSERT INTO post_body (post_markdown) VALUES ('First post body'), ('second post body');

INSERT INTO post_header (post_title, post_body_id) VALUES ('First Post', 1), ('Second Post', 2);

INSERT INTO post_tags (tag_id, post_header_id) VALUES (1,1), (2,1),(3,1), (3,2);
