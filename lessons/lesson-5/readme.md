# Lesson 5 - Normalizing Word and Phrases with Semantic Search  
In this lession we take a look at word and phrase normaliztion.  

#### Normalization
Normalization is the process of converting a word or phrase into a standard or canonical form. It often involves removing variations, redundancies, or unnecessary details to make the term consistent for comparison, storage, or further processing. This can include:        

- Singularization: Converting plural forms into their singular counterparts (e.g., ""Ducks"" → ""Duck"").
- Removing Articles or Prepositions: Stripping words like ""the,"" ""of,"" or ""in"" if they are not essential to the meaning (e.g., ""The Walking Dead"" → ""Walking Dead"").
- Standardizing Case: Ensuring consistent letter casing (e.g., lowercase or title case).
- Simplifying Structure: Reordering words or removing non-critical ones to clarify the core meaning (e.g., ""Walking Dead, the"" → ""Walking Dead"").
- Lemmatization: Converting inflected forms of words to their base or dictionary form (e.g., ""running"" → ""run"").

#### Semantic Simalarity Search using Normalized Word or Phrase
Normalization of words or phrases are often used in combination with a **semantic similarity search**.  The idea is to take a **word** or a **phrase** convert it into a normalized version then use this to retrieve records using some sort of LIKE operation and using those records leverage the LLM to perform a similarity match returning the best match of the records you shared with the LLM.  Very similar to have documents indexed in Azure AI Search and perform a semantic similarity search against those documents.  

[Click here to jump to the Lesson 5 Solution](/solutions/Semantic-Kernel-101/ConsoleApp-SK-Lesson-5)

[🔼 Home ](/README.md) | [Back Lesson 3 ▶](/lessons/lesson-3/README.md)

