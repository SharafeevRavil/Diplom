import re
import sys
from simhash import Simhash

def get_features(s):
    width = 9
    s = s.lower()
    s = re.sub(r'[^\w]+', '', s)
    return [s[i:i + width] for i in range(max(len(s) - width + 1, 1))]

input1 = sys.argv[1]
input2 = sys.argv[2]

hash1 = Simhash(get_features(input1))
hash2 = Simhash(get_features(input2))

print(f'{hash1.value:064b}')
#print(f'{hash1.value:064b}')
#print(f'{hash1:064b}')