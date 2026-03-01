import os
import sys
import time
import socket  # 중복 실행 방지를 위한 소켓 라이브러리
import pandas as pd
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

# 프로그램이 켜져 있는 동안 포트 점유를 유지할 전역 변수
lock_socket = None 

def prevent_multiple_instances():
    global lock_socket
    lock_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        # 안 쓰는 임의의 포트(예: 65432)를 쥐어봅니다.
        lock_socket.bind(('127.0.0.1', 65432))
    except socket.error:
        # 이미 다른 프로그램이 포트를 쥐고 있다면 에러가 발생하며 이곳으로 옵니다.
        print("\n" + "=" * 60)
        print("⚠️ 이미 감시 프로그램이 실행 중입니다!")
        print("기존에 켜져 있는 까만 콘솔창을 확인해 주세요.")
        print("중복 실행을 방지하기 위해 3초 뒤 창이 자동으로 닫힙니다.")
        print("=" * 60 + "\n")
        time.sleep(3)
        sys.exit() # 프로그램 안전 종료

class ExcelToCsvHandler(FileSystemEventHandler):
    # 👉 [추가된 부분] 파일별 변환 시간을 기억할 수첩(딕셔너리) 생성
    def __init__(self):
        super().__init__()
        self.last_processed = {} 

    # 파일이 새로 생성될 때 (복사/붙여넣기 등)
    def on_created(self, event):
        self.process_event(event)

    # 파일이 수정될 때 (엑셀에서 덮어쓰기 저장 등)
    def on_modified(self, event):
        self.process_event(event)

    def process_event(self, event):
        # 폴더 이벤트는 무시
        if event.is_directory:
            return
        
        filepath = event.src_path
        filename = os.path.basename(filepath)

        # .xlsx 파일만 타겟으로 하며, 엑셀 임시파일(~$로 시작)은 철저히 무시
        if filepath.endswith('.xlsx') and not filename.startswith('~$'):
            
            # 👉 [추가된 부분] 쿨타임 검사 (1.5초 이내의 중복 알림 무시)
            current_time = time.time()
            if filepath in self.last_processed:
                if current_time - self.last_processed[filepath] < 1.5:
                    return # 1.5초가 안 지났으면 아무 작업도 하지 않고 돌아감
            
            # 수첩에 현재 파일의 변환 시간을 기록/갱신
            self.last_processed[filepath] = current_time
            
            # 조건이 모두 맞으면 변환 시작
            self.convert_to_csv(filepath)

    def convert_to_csv(self, filepath):
        dir_name = os.path.dirname(filepath)
        base_name = os.path.splitext(os.path.basename(filepath))[0]
        # 같은 폴더 위치에 동일한 이름의 .csv 파일 경로 생성
        csv_filepath = os.path.join(dir_name, f"{base_name}.csv")

        print(f"👀 감지됨: {os.path.basename(filepath)} - 변환을 시도합니다...")

        max_retries = 5
        for attempt in range(max_retries):
            try:
                # pandas를 사용해 엑셀 데이터 읽기 (원본 .xlsx는 전혀 건드리지 않고 읽기만 함)
                df = pd.read_excel(filepath, engine='openpyxl')
                
                # 동일한 이름의 csv가 있으면 자동으로 덮어쓰기 진행 (index=False, utf-8)
                df.to_csv(csv_filepath, index=False, encoding='utf-8')
                
                print(f"✅ [성공] {os.path.basename(csv_filepath)} 생성/덮어쓰기 완료!\n")
                break 
                
            except PermissionError:
                print(f"⏳ 엑셀이 파일을 처리 중입니다. 1초 후 재시도... ({attempt+1}/{max_retries})")
                time.sleep(1)
            except Exception as e:
                print(f"❌ [에러] 변환 실패: {e}\n")
                break

def start_watching():
    # 본격적인 감시를 시작하기 전에 중복 실행 여부부터 확인합니다.
    prevent_multiple_instances()

    # exe로 실행되었는지, py로 실행되었는지 파악하여 정확한 최상위 디렉터리 설정
    if getattr(sys, 'frozen', False):
        watch_dir = os.path.dirname(sys.executable)
    else:
        watch_dir = os.path.dirname(os.path.abspath(__file__))

    os.chdir(watch_dir)

    event_handler = ExcelToCsvHandler()
    observer = Observer()
    
    # recursive=True로 설정하여 하위 디렉터리까지 모두 감시
    observer.schedule(event_handler, watch_dir, recursive=True)
    
    print("=" * 60)
    print(f"🕵️‍♂️ 감시 시작 경로: {watch_dir} (하위 폴더 포함)")
    print("1. 이 콘솔 창이 켜져 있는 동안 백그라운드에서 계속 동작합니다.")
    print("2. 엑셀 파일(.xlsx)이 저장/생성되면 원본은 유지하고, 동일한 폴더에 UTF-8 CSV를 만듭니다.")
    print("3. 기존 CSV 파일이 있다면 새 데이터로 덮어씁니다.")
    print("종료하시려면 이 창을 닫아주세요.")
    print("=" * 60 + "\n")
    
    observer.start()
    
    # 창이 켜져 있는 동안 계속 실행되도록 유지 (창을 닫으면 프로세스도 함께 종료됨)
    try:
        while True:
            time.sleep(1) 
    except KeyboardInterrupt:
        observer.stop()
        print("\n감시를 종료합니다.")
    observer.join()

if __name__ == "__main__":
    start_watching()