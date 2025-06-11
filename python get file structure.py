import os

def get_tree(start_path, indent=""):
    tree_output = ""
    items = os.listdir(start_path)
    for index, item in enumerate(sorted(items)):
        path = os.path.join(start_path, item)
        is_last = index == len(items) - 1
        connector = "└── " if is_last else "├── "
        tree_output += indent + connector + item + "\n"
        if os.path.isdir(path):
            extension = "    " if is_last else "│   "
            tree_output += get_tree(path, indent + extension)
    return tree_output

# Use the folder where this script is located
folder_path = os.path.dirname(os.path.abspath(__file__))

# Generate tree
tree = folder_path + "\n" + get_tree(folder_path)

# Write to a file
output_file = os.path.join(folder_path, "tree_output.txt")
with open(output_file, "w", encoding="utf-8") as f:
    f.write(tree)

print(f"Tree structure written to: {output_file}")
