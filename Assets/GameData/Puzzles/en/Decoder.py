import json
import sys
import os
import base64

try:
    import xxtea
except ImportError:
    print("✗ Error: xxtea library not found")
    print("Install it with: pip install xxtea")
    sys.exit(1)

def decrypt_field(encrypted_value, key):
    """Decrypt a single field using XXTEA"""
    try:
        # Decode from base64 first
        encrypted_bytes = base64.b64decode(encrypted_value)
        
        # XXTEA decrypt without padding (raw mode)
        decrypted = xxtea.decrypt(encrypted_bytes, key, padding=False)
        
        # Decode to UTF-8 and strip null padding bytes
        result = decrypted.decode('utf-8').rstrip('\x00')
        return result.strip()
    except Exception as e:
        print(f"Error decrypting field '{encrypted_value[:30]}...': {e}")
        return None

def decrypt_aw_field(encrypted_aw, key):
    """Decrypt the 'aw' field which contains '-' separated encrypted chunks"""
    chunks = encrypted_aw.split('-')
    decrypted_chunks = []
    
    for i, chunk in enumerate(chunks):
        decrypted = decrypt_field(chunk, key)
        if decrypted:
            decrypted_chunks.append(decrypted)
        else:
            print(f"Warning: Failed to decrypt chunk {i+1}/{len(chunks)}: {chunk[:30]}...")
    
    return '-'.join(decrypted_chunks)

def decrypt_data(data, key):
    """Decrypt all encrypted fields in the data object"""
    decrypted = {
        "i": data["i"],
        "c": data["c"],
        "r": data["r"],
        "tw": decrypt_field(data["tw"], key),
        "g": decrypt_field(data["g"], key),
        "aw": decrypt_aw_field(data["aw"], key),
        "dl": data["dl"]
    }
    return decrypted

def decrypt_json_file(input_file, key):
    """Decrypt a JSON file containing array of encrypted objects"""
    # Generate output filename
    file_dir = os.path.dirname(input_file) or "."
    file_name = os.path.basename(input_file)
    name_without_ext, ext = os.path.splitext(file_name)
    output_file = os.path.join(file_dir, f"{name_without_ext}_decoded{ext}")
    
    try:
        # Read encrypted data
        print(f"Reading file: {input_file}")
        with open(input_file, 'r', encoding='utf-8') as f:
            encrypted_data = json.load(f)
        
        # Check if it's an array or single object
        if isinstance(encrypted_data, list):
            print(f"Decrypting {len(encrypted_data)} items...")
            decrypted_data = [decrypt_data(item, key) for item in encrypted_data]
        else:
            print("Decrypting single item...")
            decrypted_data = decrypt_data(encrypted_data, key)
        
        # Write decrypted data
        with open(output_file, 'w', encoding='utf-8') as f:
            json.dump(decrypted_data, f, indent=4, ensure_ascii=False)
        
        print(f"\n✓ Successfully decrypted!")
        print(f"✓ Input:  {input_file}")
        print(f"✓ Output: {output_file}")
        
        return decrypted_data
    
    except FileNotFoundError:
        print(f"✗ Error: File '{input_file}' not found")
        sys.exit(1)
    except json.JSONDecodeError as e:
        print(f"✗ Error: Invalid JSON in '{input_file}': {e}")
        sys.exit(1)
    except Exception as e:
        print(f"✗ Error: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)

def main():
    # Your decryption key (replace with your actual key)
    DECRYPT_KEY = "Win more rewards"
    
    # Check command line arguments
    if len(sys.argv) != 2:
        print("Usage: python decoder.py /path/to/json")
        print("Example: python decoder.py data.json")
        print("\nMake sure to set DECRYPT_KEY in the script!")
        sys.exit(1)
    
    input_file = sys.argv[1]
    
    # Check if key has been set
    if DECRYPT_KEY == "your_decrypt_key_here":
        print("✗ Error: Please set DECRYPT_KEY in the script before running!")
        sys.exit(1)
    
    # Decrypt the file
    decrypt_json_file(input_file, DECRYPT_KEY)

if __name__ == "__main__":
    main()