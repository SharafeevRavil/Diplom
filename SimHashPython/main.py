import re
import sys
from simhash import Simhash

def get_features(s):
    width = 9
    s = s.lower()
    s = re.sub(r'[^\w]+', '', s)
    return [s[i:i + width] for i in range(max(len(s) - width + 1, 1))]

input = sys.argv[1]

print(f'{Simhash(get_features(input)).value:064b}')