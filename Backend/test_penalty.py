import difflib

def get_lexical_similarity(text1, text2):
    return difflib.SequenceMatcher(None, text1, text2).ratio()

test_cases = [
    ("sa duhatulo", "magpabilin", 0.86),  # Hallucination
    ("tinagan kuket jerom", "ti nagan ko ket", 0.812),  # Correct phrase with bad spelling
    ("when", "wen", 0.82), # Short phrase
    ("shak", "siak", 0.81), # Short phrase
    ("taga sadino kah", "taga sadino ka", 0.85), # Perfect phrase
    ("usa duha tulo", "duha", 0.82), # Multiple vs one
    ("maayong buntag", "maayong gabii", 0.87), # Opposite phrase, same prefix
]

print("--- Anti-Hallucination Filter Test ---")
for t1, t2, e5_score in test_cases:
    lex = get_lexical_similarity(t1, t2)
    
    final_score = e5_score
    if lex < 0.35:
        final_score -= 0.15  # Apply 15% penalty
        status = "PENALIZED (Hallucination)"
    else:
        status = "PASSED"
        
    print(f"'{t1}' vs '{t2}': Lex={lex:.2f}, Final={final_score:.2f} -> {status}")
