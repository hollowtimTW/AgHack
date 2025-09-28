# AgHack API ���

## ²��
AgHack �O�@�ӹA�~����ʴ��t�ΡA���ѧ��㪺 RESTful API �Ӧs������B�a�U���M�u�~�o�����ʴ���ơC

## API �򥻸�T
- **Base URL**: `https://localhost:7xxx/api/`
- **����**: v1
- **��Ʈ榡**: JSON
- **API ���**: �}�o���ҤU�i�z�L `/swagger` �s�� Swagger UI

## �D�n API ���I

### 1. ����ʴ� API (`/api/WaterQualityApi`)
- `GET /stations` - ���o�Ҧ��������
- `GET /stations/{id}` - ���o�S�w�����ԲӸ��
- `GET /records` - ���o�ʴ��O�� (�䴩�z��P����)
- `GET /items` - ���o�ʴ����زM��
- `GET /statistics` - ���o�έp���

### 2. �a�U���ʴ� API (`/api/GroundwaterApi`)
- `GET /stations` - ���o�Ҧ��a�U������
- `GET /stations/{id}` - ���o�S�w�����ԲӸ��
- `GET /records` - ���o�ʴ��O�� (�䴩�z��P����)
- `GET /items` - ���o�ʴ����زM��
- `GET /statistics` - ���o�έp���

### 3. �u�~�o���ʴ� API (`/api/IndustrialWastewaterApi`)
- `GET /departments` - ���o�����M��
- `GET /stations` - ���o�u�~�o������
- `GET /stations/{id}` - ���o�S�w�����ԲӸ��
- `GET /monitoringpoints` - ���o�ʴ��I�M��
- `GET /records` - ���o�ʴ��O��
- `GET /statistics` - ���o�έp���

### 4. �ѦҸ�� API (`/api/ReferenceApi`)
- `GET /counties` - ���o�����M��
- `GET /towns` - ���o�m���ϲM��
- `GET /basins` - ���o�y��M��
- `GET /rivers` - ���o�e�t�M��
- `GET /water-quality-categories` - ���o����ʴ����ؤ���
- `GET /groundwater-categories` - ���o�a�U���ʴ����ؤ���
- `GET /all-stations-summary` - ���o�Ҧ������έp�K�n

### 5. �j�M�d�� API (`/api/SearchApi`)
- `GET /stations` - �j�M���� (����r�B�a�Ͽz��)
- `GET /nearby-stations` - �j�M������� (�y�зj�M)
- `GET /recent-data` - ���o�̪�ʴ����

## �ϥνd��

### ���o�Ҧ��������
```
GET /api/WaterQualityApi/stations
```

### ���o�S�w�������ʴ��O�� (����)
```
GET /api/WaterQualityApi/records?stationId=1&page=1&pageSize=50
```

### �j�M������� (�H�y�Ь����ߡA5�����d��)
```
GET /api/SearchApi/nearby-stations?latitude=25.047924&longitude=121.517081&radiusKm=5
```

### ���o�̪� 7 �Ѫ�������
```
GET /api/SearchApi/recent-data?dataType=WQ&days=7&limit=100
```

## ���~�B�z
�Ҧ� API ���]�t�A�����~�B�z�A�|�^�ǼзǪ� HTTP ���A�X�G
- `200 OK` - ���\
- `404 Not Found` - �䤣��귽
- `500 Internal Server Error` - ���A���������~

���~�^���榡�G
```json
{
    "message": "���~�y�z",
    "error": "�Բӿ��~�T��"
}
```

## �����䴩
�䴩������ API �|�^�ǥH�U�榡�G
```json
{
    "data": [...],
    "pagination": {
        "page": 1,
        "pageSize": 100,
        "totalCount": 1500,
        "totalPages": 15
    }
}
```

## �ϥ� Swagger UI
�b�}�o���Ҥ��A�i�H�z�L�s�����s�� `https://localhost:7xxx/swagger` �Өϥ� Swagger UI �����A���ѡG
- ���㪺 API ���
- ���ʦ� API ����
- �ШD/�^���d��
- API �������