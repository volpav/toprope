import argparse
from os import listdir
from os.path import isfile, isdir, join
import xml.etree.ElementTree as ElementTree

total_routes = 0

def traverse_path(path):
	global total_routes

	# Getting all files within the given directory
	files = [(f, join(path, f)) for f in listdir(path) if isfile(join(path, f))]

	for f in files:
		# Routes are saved in 'routes.xml'
		if f[0] == 'routes.xml':
			routes = 0
			tree = ElementTree.parse(f[1])

			# Collecting the number of routes
			for sub in tree.iter('route'):
				routes += 1

			#print(f[1], '-->', routes)

			# Updating the total number of routes
			total_routes += routes

	# Getting all sub-directories
	dirs = [d for d in listdir(path) if isdir(join(path, d))]

	for d in dirs:
		traverse_path(join(path, d))


parser = argparse.ArgumentParser(description = 'Counts all routes within the given directory.')
parser.add_argument('dir', metavar = 'directory', help = 'Base directory.')

args = parser.parse_args()

traverse_path(args.dir)
print('Total routes:', total_routes)

