import difflib

def get_lexical_similarity(text1, text2):
    return difflib.SequenceMatcher(None, text1, text2).ratio()

test_cases = [
    ("sa duhatulo", "magpabilin"),  # The hallucination
    ("tinagan kuket jerom", "ti nagan ko ket"),  # The correct Ilocano match
    ("naimbag ah bigat", "naimbag a bigat"), # Correct Ilocano
    ("usa duha tulo", "duha"), # Multiple numbers vs single number
]

for t1, t2 in test_cases:
    score = get_lexical_similarity(t1, t2)
    print(f"'{t1}' vs '{t2}' -> Lexical Score: {score:.2f}")
