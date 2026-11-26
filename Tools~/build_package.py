import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import tarfile

parser = argparse.ArgumentParser()
parser.add_argument('--output', required=True, type=Path)
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
manifest = json.loads((root / 'package.json').read_text())
tracked = subprocess.check_output(['git', '-C', str(root), 'ls-files', '-z']).decode().split('\0')
tracked = sorted(path for path in tracked if path)
for folder in ('Runtime/', 'Editor/', 'Fonts~/English/', 'Fonts~/Persian/', 'Fonts~/Turkish/'):
    if not any(path.startswith(folder) for path in tracked):
        raise SystemExit('Missing package content: ' + folder)
args.output.mkdir(parents=True, exist_ok=True)
archive = args.output / (manifest['name'] + '-' + manifest['version'] + '.tgz')
with tarfile.open(archive, 'w:gz', format=tarfile.PAX_FORMAT) as output:
    for relative in tracked:
        output.add(root / relative, arcname='package/' + relative, recursive=False)
with tarfile.open(archive) as packed:
    names = packed.getnames()
    if any(not name.startswith('package/') for name in names):
        raise SystemExit('Invalid UPM layout')
    packaged = json.load(packed.extractfile('package/package.json'))
    if packaged['version'] != manifest['version']:
        raise SystemExit('Version mismatch')
print(json.dumps({'version': manifest['version'], 'archive': str(archive), 'files': len(names), 'bytes': archive.stat().st_size, 'sha256': hashlib.sha256(archive.read_bytes()).hexdigest()}, indent=2))
